namespace Synthesizer

open System.IO


type writeWav( 
        ?sampleRate0:int, 
        ?pcmFormat0:int16,
        ?nbChannels0:int,
        ?bytesPerSample0:int
        ) =

    let sampleRate = (defaultArg sampleRate0 44100) 
    let pcmFormat = (defaultArg pcmFormat0 1s)
    let nbChannels = (defaultArg nbChannels0 1)
    let bytesPerSample = (defaultArg bytesPerSample0 2)

    let bitsPerSample = bytesPerSample * 8



    member x.Write stream (data: byte []) =
        let byteRate = sampleRate * nbChannels * bytesPerSample
        let blockAlign = uint16 (nbChannels * bytesPerSample)
        
        use writer = new BinaryWriter(stream)
        // RIFF
        writer.Write("RIFF"B)
        writer.Write(36 + data.Length) // File size
        writer.Write("WAVE"B)
        // fmt
        writer.Write("fmt "B)
        writer.Write(16) // Header size
        writer.Write(pcmFormat)
        writer.Write(uint16 nbChannels)
        writer.Write(sampleRate)
        writer.Write(byteRate)
        writer.Write(blockAlign)
        writer.Write(uint16 bitsPerSample)
        // data
        writer.Write("data"B)
        writer.Write(data.Length)
        writer.Write(data)


    (* let generate func =
        let size = int (duration * float sampleRate)
        let toBytes x =
            let unitary = (x + 2. ** (float bytesPerSample - 1.)) / 2.
            let upscaled = round (unitary * (256. ** (float bytesPerSample))) - (if unitary = 1. then 1. else 0.)
            [ for k in 0..(bytesPerSample-1) do byte (upscaled/(256.**k)) ]
        
        let getData = float >> (fun x -> (x / float sampleRate)) >> func freq amplitude >> overD.makeOverdrive overdrive >> toBytes
        [ for i in 0 .. (size - 1) do yield! getData i ] |> Array.ofList
    
    let streamTriangle = File.Create("toneTriangle.wav")
    let dataTriangle = Array.init 44100 (fun i -> 
        makeOverdrive(
        triangleWave (float i/44100.)1. 1.) 0.8 
        |> sample)
    *)
    