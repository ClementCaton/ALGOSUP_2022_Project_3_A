open System.IO

let pi = 3.141592653589793

let makeOverdrive x multiplicator=
    if x < (-1. * multiplicator) then (-1. * multiplicator) else
    if x > 1. * multiplicator then 1. * multiplicator else
    x

let sinWave x frequence amplitude =
    amplitude * sin(2. * pi * x * frequence)

let sawWave x frequence amplitude =
    2. * amplitude * ( x * frequence - floor(0.5 +  x * frequence))

let squareWave x frequence amplitude= 
    amplitude * float(sign(sin(2. * pi * x * frequence)))

let triangleWave x frequence amplitude= 
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

let stream = File.Create("tone.wav")
let data = Array.init 44100 (fun i -> 
    (sinWave (float i/44100.) 4. 0.25) + 
    (sawWave (float i / 44100.) 4. 0.25) +
    (squareWave (float i/44100.) 4. 0.25) + 
    (triangleWave (float i / 44100.) 4. 0.25)
    |> sample)

let streamSin = File.Create("toneSin.wav")
let dataSin = Array.init 44100 (fun i -> 
    makeOverdrive(
        sinWave (float i/44100.)1. 1.) 0.8 
        |> sample)


let streamSaw = File.Create("toneSaw.wav")
let dataSaw = Array.init 44100 (fun i -> 
    makeOverdrive(
        sawWave (float i/44100.)1. 1.) 0.8 
        |> sample)


let streamSquare = File.Create("toneSquare.wav")
let dataSquare = Array.init 44100 (fun i -> 
    makeOverdrive(
        squareWave (float i/44100.)1. 1.) 0.8 
        |> sample)

let streamTriangle = File.Create("toneTriangle.wav")
let dataTriangle = Array.init 44100 (fun i -> 
    makeOverdrive(
        triangleWave (float i/44100.)1. 1.) 0.8 
        |> sample)

write stream data
write streamSin dataSin
write streamSaw dataSaw
write streamSquare dataSquare
write streamTriangle dataTriangle