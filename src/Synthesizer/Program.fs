namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =

    let basicSound = API.note (Seconds 3) Note.A 4
    let flanger = Filter.primitiveFlanger 10 44100. basicSound
    // let reverb = Filter.reverb basicSound 50 0.8 0.2 44100.
    // let echo = Filter.reverb basicSound 5 0.7 1. 44100.
    API.writeToWav "basic.wav" basicSound
    API.writeToWav "flanger.wav" flanger
    // API.writeToWav "reverb.wav" reverb
    // API.writeToWav "echo.wav" echo
    // Custom duration
    let DottedEighth = Custom (1./8. * 1.5)
    let EighthAndHalf = Custom (1./8. + 1./2.)

    // Create the melodies
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

    let mono = [music]
    API.writeToWav "rickroll.wav" mono

