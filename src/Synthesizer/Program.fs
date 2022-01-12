// namespace Synthesizer

open System
open System.IO
open SFML.Audio
open SFML.System
open System.Diagnostics // needed to play song on MAC OS

// module program = 
type OS =
        | OSX            
        | Windows
        | Linux

let getOS = 
        match int Environment.OSVersion.Platform with
        | 4 | 128 -> Linux // 4 is the reference for Unix
        | 6       -> OSX // 6 for OSX
        | 2       -> Windows // 2 for Windows 

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

    let writer = new BinaryWriter(stream)   // Do not use a "use" instead of the let because it will close the stream
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
        [ for k in 0..(bytesPerSample-1) do byte (upscaled/(256.**(float(k)))) ]

    let getData = float >> (fun x -> (x / float sampleRate)) >> func freq amplitude >> overD.makeOverdrive overdrive >> toBytes
    [ for i in 0 .. (size - 1) do yield! getData i ] |> Array.ofList 


// write (File.Create("toneSin.wav")) (generate fourWaves.sinWave)
// write (File.Create("toneSquare.wav")) (generate fourWaves.squareWave)
// write (File.Create("toneTriangle.wav")) (generate fourWaves.triangleWave)
// write (File.Create("toneSaw.wav")) (generate sawWave)

using (new MemoryStream()) (fun stream ->
    write stream (generate fourWaves.sawWave)
    playSound.playWithOffset stream (float32(0.9))
    )

using (new MemoryStream()) (fun stream ->
    write stream (generate fourWaves.sawWave)
    playSound.playWithOffset stream (float32(0.5))
    )

using (new MemoryStream()) (fun stream ->
    write stream (generate fourWaves.sawWave)
    playSound.play stream
    )

// Process.Start("afplay", "toneDouble.wav") //use this to play sound in OSX