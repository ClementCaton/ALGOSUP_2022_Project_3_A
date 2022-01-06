open System.IO

let pi = 3.141592653589793

let sinWave x frequence amplitude =
    let buffer = frequence * 148.
    amplitude * sin(2. * pi * x * buffer)

let sawWave x frequence amplitude =
    let buffer = 8000./frequence
    2. * amplitude * ( x / buffer - floor(0.5 +  x / buffer))

let squareWave x frequence amplitude = 
    let buffer = frequence * 148.
    amplitude * float(sign(sin(2. * pi * x * buffer)))

let triangleWave x frequence amplitude = 
    let buffer = frequence * 148.
    2. * amplitude * asin(sin(2. * pi * x * buffer)) / pi

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

let dataSin = Array.init 8000 (fun i -> sinWave i 1. 1. |> sample)
let streamSin = File.Create("toneSin.wav")

let dataSaw = Array.init 8000 (fun i -> sawWave i 1. 1. |> sample)
let streamSaw = File.Create("toneSaw.wav")

let dataSquare = Array.init 8000 (fun i -> squareWave i 1. 1. |> sample)
let streamSquare = File.Create("toneSquare.wav")

let dataTriangle = Array.init 8000 (fun i -> triangleWave i 1. 1. |> sample)
let streamTriangle = File.Create("toneTriangle.wav")

write streamSin dataSin
write streamSaw dataSaw
write streamSquare dataSquare
write streamTriangle dataTriangle