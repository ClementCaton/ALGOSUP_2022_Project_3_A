namespace Synthesizer

open System.IO

module Program =
    let test = createSoundData(frequency0 = (API.getNoteFreq Note.A 4), duration0 = Half, bpm0 = 114).createFromDataPoints Sin [(0., 0.); (1., 1.); (3., 0.5); (5, 0.2)]
    let test2 = createSoundData(frequency0 = (API.getNoteFreq Note.A 4), duration0 = Custom 3, bpm0 = 114).creteWithEnvelope Sin 0.5 0.5 0.1 1. 0.3
    let test3 = API.note (Custom 2) Note.A 4
    let test4 = Filter.enveloppe test3 44100. 0.5 0.1 0. 0.3 0.5
    API.writeToWav "wave3.wav" test3
    API.writeToWav "wave4.wav" test4

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
