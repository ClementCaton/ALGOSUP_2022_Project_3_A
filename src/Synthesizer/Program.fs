// namespace Synthesizer

// module Program =
//     let t = API.getNoteOffset 4 "C" 330
//     printfn $"test= {t}" 
    /////////////////////////////////////////////////////////////////////////////////
open System
open System.IO
open SFML.Audio

let freq = 440. // In Hertz
let sampleRate = 44100 // In Hertz
let amplitude = 0.8
let overdrive = 0.9
let duration = 1. // In seconds
let pcmFormat = 1s
let nbChannels = 1
let bytesPerSample = 2

module overD =
    let makeOverdrive multiplicator x =
        if x < (-1. * multiplicator) then (-1. * multiplicator) else
        if x > 1. * multiplicator then 1. * multiplicator else
        x

let bitsPerSample = bytesPerSample * 8

/// Write WAVE PCM soundfile
let write stream (data: byte []) =
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

let generate func =
    let size = int (duration * float sampleRate)
    let toBytes x =
        let unitary = (x + 2. ** (float bytesPerSample - 1.)) / 2.
        let upscaled = round (unitary * (256. ** (float bytesPerSample))) - (if unitary = 1. then 1. else 0.)
        [ for k in 0..(bytesPerSample-1) do byte (upscaled/(256.**k)) ]

    let getData = float >> (fun x -> (x / float sampleRate)) >> func freq amplitude >> overD.makeOverdrive overdrive >> toBytes
    [ for i in 0 .. (size - 1) do yield! getData i ] |> Array.ofList

write (File.Create("toneSin.wav")) (generate fourWaves.sinWave)
write (File.Create("toneSquare.wav")) (generate fourWaves.squareWave)
write (File.Create("toneTriangle.wav")) (generate fourWaves.triangleWave)
write (File.Create("toneSaw.wav")) (generate fourWaves.sawWave)