open System.IO

let pi = 3.141592653589793

let sinWave x frequence amplitude =
    amplitude * sin(2. * pi * x * frequence)

let sawWave x frequence amplitude =
    2. * amplitude * ( x * frequence - floor(0.5 +  x * frequence))

let squareWave x frequence amplitude = 
    amplitude * float(sign(sin(2. * pi * x * frequence)))

let triangleWave x frequence amplitude = 
    2. * amplitude * asin(sin(2. * pi * x * frequence)) / pi

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
    let sampleRate = 44100 in writer.Write(sampleRate)
    let byteRate = sampleRate in writer.Write(byteRate)
    let blockAlign = 1s in writer.Write(blockAlign)
    let bitsPerSample = 8s in writer.Write(bitsPerSample)
    // data
    writer.Write("data"B)
    writer.Write(data.Length)
    writer.Write(data)

let sample x = (x + 1.)/2. * 255. |> byte 
let data = Array.init 44100 (fun i -> 
    (sinWave (float i/44100.) 4. 0.25) + 
    (sawWave (float i / 44100.) 4. 0.25) +
    (squareWave (float i/44100.) 4. 0.25) + 
    (triangleWave (float i / 44100.) 4. 0.25)
    |> sample)
let stream = File.Create("tone.wav")

let dataSin = Array.init 44100 (fun i -> sinWave (float i/44100.) 1. 1. |> sample)
let streamSin = File.Create("toneSin.wav")

let dataSaw = Array.init 44100 (fun i -> sawWave (float i/44100.) 1. 1. |> sample)
let streamSaw = File.Create("toneSaw.wav")

let dataSquare = Array.init 44100 (fun i -> squareWave (float i/44100.) 1. 1. |> sample)
let streamSquare = File.Create("toneSquare.wav")

let dataTriangle = Array.init 44100 (fun i -> triangleWave (float i/44100.) 1. 1. |> sample)
let streamTriangle = File.Create("toneTriangle.wav")

write stream data
write streamSin dataSin
write streamSaw dataSaw
write streamSquare dataSquare
write streamTriangle dataTriangle
