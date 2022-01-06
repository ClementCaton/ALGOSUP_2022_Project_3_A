open System.IO

let squareWave x amplitude frequence= 
    amplitude * sign(sin(((2. * 3.149 * x * frequence)))) 

let triangleWave x  amplitude frequence= 
    ((amplitude)/(3.14)) * asin(sin((3.14*2.*frequence) * x))

/// Write WAVE PCM soundfile (8KHz Mono 8-bit)
let write stream (data:byte[]) =
    use writer = new BinaryWriter(stream)
    // RIFF
    writer.Write("RIFF"B)
    let size = 36 + data.Length in writer.Write(size)
    writer.Write("WAVE"B)
    // fmt
    writer.Write("fmt "B)
    let headerSize = 16 in writer.Write(headerSize)
    let pcmFormat = 1s in writer.Write(pcmFormat)
    let mono = 1s in writer.Write(mono)
    let sampleRate = 8000 in writer.Write(sampleRate)
    let byteRate = sampleRate in writer.Write(byteRate)
    let blockAlign = 1s in writer.Write(blockAlign)
    let bitsPerSample = 8s in writer.Write(bitsPerSample)
    // data
    writer.Write("data"B)
    writer.Write(data.Length)
    writer.Write(data)

let sample x = (x + 1.)/2. * 255. |> byte 

let data = Array.init 16000 (fun i -> (2. * (((float i)/(40.))-floor(((1.)/(2.))+((float i)/(40.))))) + ((2.)/(3.14))* asin(3.*sin(3.14 * float i)) |> sample)
let stream = File.Create("tonje.wav")
write stream data