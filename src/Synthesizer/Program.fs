namespace Synthesizer

open System.IO

module Program =
    // Custom duration
    let DottedEighth = Custom (3./16.)
    let DottedHalf = Custom (5./8.)

    let mainMelody = API.compose [
        API.note Eighth Note.D 4
        API.note Eighth Note.E 4
        API.note Eighth Note.F 4
        API.note Eighth Note.F 4
        API.note Eighth Note.G 4
        API.note DottedEighth Note.E 4
        API.note Sixteenth Note.D 4
        API.note DottedHalf Note.C 4
    ]

    let secondMelody = API.compose [
        API.note Half Note.Bb 3
        API.silence Eighth
        API.note DottedEighth Note.C 4
    ]

    // Superpose the melodies and write to file
    //let music = API.add [mainMelody; secondMelody]
    API.writeToWav "wave.wav" mainMelody //music

/// Write WAVE PCM soundfile


//write (File.Create("toneSquare.wav")) (generate fourWaves.squareWave)
//write (File.Create("toneTriangle.wav")) (generate fourWaves.triangleWave)
//write (File.Create("toneSaw.wav")) (generate fourWaves.sawWave)