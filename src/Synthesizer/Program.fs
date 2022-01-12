// namespace Synthesizer

open System
open System.IO
open SFML.Audio
open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open Synthesizer

type OS =
        | OSX            
        | Windows
        | Linux

// let getOS = 
//     match int Environment.OSVersion.Platform with
//     | 4 | 128 -> Linux // 4 is the reference for Unix
//     | 6       -> OSX // 6 for OSX
//     | 2       -> Windows // 2 for Windows 

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


// write (File.Create("toneSquare.wav")) (generate fourWaves.squareWave)
// write (File.Create("toneTriangle.wav")) (generate fourWaves.triangleWave)
// write (File.Create("toneSaw.wav")) (generate sawWave)
let writer = new writeWav()
// writer.Write (File.Create("toneSin.wav")) (writer.generate fourWaves.sinWave)
// using (new MemoryStream()) (fun stream ->
//     writer.Write stream (writer.generate fourWaves.sawWave)
//     playSound.playWithOffset stream (float32(0.9))
//     )

// using (new MemoryStream()) (fun stream ->
//     writer.Write stream (writer.generate fourWaves.sawWave)
//     playSound.playWithOffset stream (float32(0.5))
//     )

// using (new MemoryStream()) (fun stream ->
//     writer.Write stream (writer.generate fourWaves.sawWave)
//     playSound.play stream
//     )

// using (new MemoryStream()) (fun stream ->
    // writer.Write stream (writer.generate fourWaves.sawWave)
    // )
playSound.playWithOffsetFromPath "./toneSaw.wav" (float32 0.)

// Process.Start("afplay", "toneDouble.wav") //use this to play sound in OSX