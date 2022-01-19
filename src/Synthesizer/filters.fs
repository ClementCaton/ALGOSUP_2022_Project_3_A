namespace Synthesizer

module Filter =
    open System

    let makeOverdrive multiplicator (x:List<float>) =
        [for i in x do 
            if i < (-1. * multiplicator * 256.) then (-1. * multiplicator * 256.) else
            if i > (1. * multiplicator  * 256.) then (1. * multiplicator * 256.) else
            i]

    let changeAmplitude multiplicator (x:List<float>) =
        x |> List.map (( * ) multiplicator)

    let addTwoWaves (x:List<float>) (y:List<float>) ratio = 
        let mutable output = List.empty
        let mutable oldMax = x |> List.max
        if (oldMax < (y |> List.max)) then (oldMax <- (y |> List.max))

        if not (x.Length = y.Length) then
            let diff = Math.Abs(x.Length - y.Length)
            let endArray = [for i in [0 .. diff] do 0.0]
            if x.Length > y.Length then
                let newY = List.append y endArray
                output <- List.init x.Length (fun i -> (x[i] * ratio) + (newY[i] * (1.-ratio)))
            else 
                let newX = List.append x endArray
                output <- List.init y.Length (fun i -> (newX[i] * ratio) + (y[i] * (1.-ratio)))
        else 
            output <- List.init x.Length (fun i -> (x[i] * ratio) + (y[i] * (1.-ratio)))
        output <- changeAmplitude (1./(output|>List.max)) output
        output <- changeAmplitude oldMax output
        makeOverdrive 1. output

    let createEcho (x:List<float>) (startIndex:int) (endIndex:int) (delay:float) (nbEcho:int) = //takes the whole sound and echoes it
        let silenceDelay = [for i in 0. .. delay do 0.]
        //let silenceEcho = [for i in 0 .. ( endIndex - startIndex ) do 0.]
        let echoSample = x[startIndex..endIndex]

        let mutable (output:List<List<float>>) = List.empty
        let mutable buffer = List.empty

        for i in [0 .. nbEcho] do
            buffer <- List.empty
            for a in [0 .. i] do
                buffer <- buffer |> List.append silenceDelay
                //buffer <- buffer |> List.append silenceEcho
            buffer <- List.append buffer echoSample
            output <- output @ [buffer]

        let mutable returnValue = output[0]
        for i in [(output.Length - 1).. -1 ..1] do 
            returnValue <- addTwoWaves returnValue output[i] 0.66
        let silence = [for i in 0 .. (startIndex - 1) do 0.]
        returnValue <- List.append silence returnValue
        addTwoWaves x returnValue 0.66


    type CircularBuffer<'T>(n, lag, initFunc : int -> 'T) = 
        let buffer = Array.init n initFunc
        let size = n

        let calcPos m current = 
          let toPos = m + current
          if toPos >= size then toPos % size
          else if toPos < 0 then (size + toPos % size) % size
          else toPos

        let mutable posW = 0
        // read position lags behind write position by lag
        let mutable posR = calcPos (-lag) posW
        let mutable numSlot = n

        let moveIndex m = 
          posW <- calcPos m posW
          posR <- calcPos m posR

        /// do printfn "New instance of CircularBuffer"
        do 
          if lag < 0 then failwith "Lag must be larger than or equal to zero"

        ///
        /// <summary>Push an item into the circular buffer</summary>
        ///
        member t.Push item = 
          buffer.[posW] <- item
          moveIndex 1
          if numSlot > 0 then numSlot <- numSlot - 1

        ///
        /// <summary>Check if the buffer is fully populated</summary>
        ///
        member t.IsFull() = numSlot <= 0

        ///
        /// <summary>Get the value currently being pointed to and will be replaced
        /// by a call to push</summary>
        ///
        member t.Get() = buffer.[posR]

        ///
        /// <summary>Get the value of the current read position offset by n
        /// </summary>
        ///
        member t.GetOffset n = buffer.[(calcPos n posR)]

        ///
        /// <summary>Indexer relative to the current read pointer.  Syntactic sugar
        /// for Get() and GetOffset(n)</summary>
        ///
        member t.Item idx = 
          if idx = 0 then t.Get()
          else t.GetOffset(idx)

        ///
        /// <summary>Returns a copy of the raw buffer as an array</summary>
        ///
        member t.GetRawBuffer() = buffer

        /// <summary>
        /// Returns a sequence which starts from the element in PosR and ends at
        /// PosR + lag
        /// </summary>
        member t.GetBufferSeq() = 
          seq { 
            for i in [ 0..(lag - 1) ] do
              let cur = calcPos i posR
              yield buffer.[cur]
          }

        member t.GetBuffer() = t.GetBufferSeq() |> Seq.toArray

        ///
        /// <summary>Returns the current write index.  The item the current index is
        /// pointing at will be replaced with a new item by a call to Push()
        /// </summary>
        ///
        member t.CurrentWrite() = posW

        ///
        /// <summary>Returns the current read index.  The item the current index is
        /// pointing at will be returned by a call to Get()</summary>
        ///
        member t.CurrentRead() = posR

        ///
        /// <summary>Increase the spread between write index and read index
        /// i.e. move the read index backwards</summary>
        /// <param name="n">the number of items to move.  If n is positive, the
        /// spread between read and write is increased, i.e. the read index is moved
        /// backward from its current position.  Otherwise if n is negative, the
        /// spread is decreased and the read index is moved forward to be closer to
        /// the write index</param>
        ///
        member t.AddLag n = posR <- calcPos n posR

        ///
        /// <summary>Set the lag in terms of number of samples between the read 
        /// index and the write index</summary>
        ///
        member t.SetLag n = posR <- calcPos -(abs n) posW
  
  /// <summary>
  /// Convenience function to calculate buffer size based on sampling frequency
  /// and duration and create a CircularBuffer of that size
  /// </summary>
  /// <param name="fs">Sampling frequency in Hz</param>
  /// <param name="duration">In number of seconds</param>
  /// <param name="lag"></param>
  /// <param name="init">init function</param>
  /// <returns>A circular buffer object</returns>
    let makeCircularBuffer (fs : float) (duration : float) init (delayMs : float) = 
        new CircularBuffer<float>(int (fs * duration), int (fs * delayMs / 1000.0), init)


    let modDelay fs bufferSec delayMs gain feedback wet lfo = 
        if wet < 0.0 || wet > 1.0 then failwith "wet must be between 0.0 and 1.0"
        if bufferSec * 1000.0 < delayMs then failwith "buffer size not large enough"
        let bufferSize = int (fs * bufferSec)
        let delaySamples = delayMs / 1000.0 * fs
        let buffer = CircularBuffer(bufferSize, 0, fun _ -> 0.0)
        let n = ref 0
        fun sample -> 
            n := !n + 1
            let t = (float !n) / fs
          // Add an extra one sample to the delay to make sure there is one sample 
          // ahead for linear interpolation
            let d = (lfo t) * delaySamples + 1.0
            let d' = ceil d
            let frac = d' - d
            buffer.SetLag(int d')
          // printfn "frac: %f d: %f d': %f x0: %f x1: %f v: %f" frac d d' (buffer.Get() )
          //  (buffer.GetOffset 1) (buffer.Get() * (1.0 - frac) + (buffer.GetOffset 1) * frac)
            let yn = 
                if abs (d - 1.0) < 0.0000001 then sample
                else buffer.[0] * (1.0 - frac) + (buffer.[1]) * frac

            let xn = sample
            buffer.Push(gain * xn + feedback * yn)
            wet * yn + (1.0 - wet) * sample

    let sinusoid a f ph t = 
        let w = 2.0 * System.Math.PI * f
        a * cos (w * t + ph)

    let lfo f phase depth t = 
        // short circuit for depth = 0.0
        if depth = 0.0 then 1.0
        else ((sinusoid 1.0 f phase t) + 1.0) * 0.5 * depth + (1.0 - depth)

    let flanger fs maxDelayMs feedback wet sweepFreq = 
        let bufferSec = maxDelayMs / 1000.0 * 2.0
        modDelay fs bufferSec maxDelayMs 1.0 feedback wet (lfo sweepFreq System.Math.PI 1.0)
