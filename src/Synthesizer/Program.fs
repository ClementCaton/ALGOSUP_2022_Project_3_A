namespace Synthesizer

open System.IO

module Program =
    let melody = API.compose [
        API.note Whole Note.A 4
        API.silence Quarter
        API.note Half Note.C 4
    ]
    //API.writeToWav "wave.wav" melody

/// Write WAVE PCM soundfile


//write (File.Create("toneSquare.wav")) (generate fourWaves.squareWave)
//write (File.Create("toneTriangle.wav")) (generate fourWaves.triangleWave)
//write (File.Create("toneSaw.wav")) (generate fourWaves.sawWave)