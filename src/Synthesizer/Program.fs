open System.IO

let sinWave x frequence amplitude =
    amplitude * sin(float frequence * x * 3.14159)

let sawWave x frequence amplitude =
    2. * amplitude * ( ( (x)/(float frequence) ) - floor( ( (1.) / (2.) ) + ( (x) / (float frequence) ) ) )

let squareWave x frequence amplitude = 
    amplitude * float(sign(sin(((2. * 3.149 * x * float frequence)))))

let triangleWave x frequence amplitude = 
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

let dataSin = Array.init 16000 (fun x -> sinWave x 200. 1. |> sample)
let streamSin = File.Create("toneSin.wav")

let dataSaw = Array.init 16000 (fun x -> sawWave x 200. 1. |> sample)
let streamSaw = File.Create("toneSaw.wav")

let dataSquare = Array.init 16000 (fun x -> squareWave x 200. 1. |> sample)
let streamSquare = File.Create("toneSquare.wav")

let dataTriangle = Array.init 16000 (fun x -> triangleWave x 200. 1. |> sample)
let streamTriangle = File.Create("toneTriangle.wav")

write streamSin dataSin
write streamSaw dataSaw
write streamSquare dataSquare
write streamTriangle dataTriangle
