
open System
open System.IO
open SFML.Audio
open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



type OS =
        | OSX            
        | Windows
        | Linux
        | Other

let getOS = 
    match int Environment.OSVersion.Platform with
    | 4 | 128 -> Linux // 4 is the reference for Unix
    | 6       -> OSX // 6 for OSX
    | 2       -> Windows // 2 for Windows 
    | _       -> Other // default

module Program =

    // Custom duration
    let DottedEighth = Custom (1./8. * 1.5)
    let EighthAndHalf = Custom (1./8. + 1./2.)

    let mainMelody = API.compose [
        API.note Eighth Note.D 4
        API.note Eighth Note.E 4
        API.note Eighth Note.F 4
        API.note Eighth Note.F 4
        API.note Eighth Note.G 4
        API.note DottedEighth Note.E 4
        API.note Sixteenth Note.D 4
        API.note EighthAndHalf Note.C 4
    ]

    let secondMelody = API.compose [
        API.note Half Note.Bb 3
        API.silence Eighth
        API.note DottedEighth Note.C 4
    ]

    let secondHandHigh = API.compose [
        API.note EighthAndHalf Note.Bb 2
        API.note Half Note.C 3
    ]

    let secondHandLow = API.compose [
        API.note EighthAndHalf Note.Bb 1
        API.note Half Note.C 2
    ]

    // Superpose the melodies and write to file
    let music = API.add [mainMelody; secondMelody; secondHandHigh; secondHandLow]
    API.writeToWav "wave.wav" music

//frequence amplitude verticalShift phaseShift t
    let input = API.add [API.note Whole Note.A 3;API.note Whole Note.A 4;API.note Whole Note.A 5]
    API.writeToWav "A345.wav" input
    let output = frequencyAnalysis.fourier input
    previewarr.chart output |> ignore


/// Write WAVE PCM soundfile


//write (File.Create("toneSquare.wav")) (generate fourWaves.squareWave)
//write (File.Create("toneTriangle.wav")) (generate fourWaves.triangleWave)
//write (File.Create("toneSaw.wav")) (generate fourWaves.sawWave)

// write (File.Create("toneSquare.wav")) (generate fourWaves.squareWave)
// write (File.Create("toneTriangle.wav")) (generate fourWaves.triangleWave)
// write (File.Create("toneSaw.wav")) (generate sawWave)
// let writer = new writeWav()
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


// Process.Start("afplay", "toneDouble.wav") //use this to play sound in OSX

//playMusic.playWithOffsetFromPath "./sound.wav" (float32 0.)