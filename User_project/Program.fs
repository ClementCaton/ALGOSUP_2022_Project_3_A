namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =

    let basicSound = Utility.cutCorners 20000 (Synth.note (Seconds 1) Note.A 4)
    let reverb = Filter.Reverb 0.4 0.5 0.9 44100. basicSound
    let echo = Filter.Repeater 5 0.9 1.2 44100. basicSound
    Synth.writeToWav "basic.wav" [basicSound]
    Synth.writeToWav "reverb.wav" [reverb]
    Synth.writeToWav "echo.wav" [echo]

    // // Custom duration
    // let DottedEighth = Custom (1./8. * 1.5)
    // let EighthAndHalf = Custom (1./8. + 1./2.)

    // // Create the melodies
    // let mainMelody = Synth.compose [
    //     Synth.note Eighth Note.D 4
    //     Synth.note Eighth Note.E 4
    //     Synth.note Eighth Note.F 4
    //     Synth.note Eighth Note.F 4
    //     Synth.note Eighth Note.G 4
    //     Synth.note DottedEighth Note.E 4
    //     Synth.note Sixteenth Note.D 4
    //     Synth.note EighthAndHalf Note.C 4
    // ]

    // let secondMelody = Synth.compose [
    //     Synth.note Half Note.Bb 3
    //     Synth.silence Eighth
    //     Synth.note DottedEighth Note.C 4
    // ]

    // let secondHandHigh = Synth.compose [
    //     Synth.note EighthAndHalf Note.Bb 2
    //     Synth.note Half Note.C 3
    // ]

    // let secondHandLow = Synth.compose [
    //     Synth.note EighthAndHalf Note.Bb 1
    //     Synth.note Half Note.C 2
    // ]

    // // Superpose the melodies and write to file
    // let music = Synth.add [mainMelody; secondMelody; secondHandHigh; secondHandLow]


    // Synth.writeToWav "wave.wav" [music]

    // //frequence amplitude verticalShift phaseShift t
    // let input = Synth.add [Synth.note Whole Note.A 3;Synth.note Whole Note.A 4;Synth.note Whole Note.A 5]
    // Synth.writeToWav "A345.wav" [input]
    // let output = frequencyAnalysis.fourier input
    // Synth.preview "A 3,4,5 Analysis" output |> ignore

    // let mono = [music]
    // Synth.writeToWav "rickroll.wav" mono

    // let stereo = [music; mainMelody]
    // Synth.writeToWav "rickrollStereo.wav" stereo
