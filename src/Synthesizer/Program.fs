namespace Synthesizer

open System.IO

module Program =
    API.note 6 "A"

/// Write WAVE PCM soundfile


//write (File.Create("toneSquare.wav")) (generate fourWaves.squareWave)
//write (File.Create("toneTriangle.wav")) (generate fourWaves.triangleWave)
//write (File.Create("toneSaw.wav")) (generate fourWaves.sawWave)