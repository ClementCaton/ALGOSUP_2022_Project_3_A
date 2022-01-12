namespace Synthesizer

open System
open System.IO

module API = 
    
    let getNoteFreq octav note =
        CalcNoteFreq(octav, note).Output

    let getNoteFreqOffset octav note aFourFreq =
        CalcNoteFreq(octav, note, aFourFreq).Output


    let createSound freq duration waveType=
        let creator = createSoundData(frequency0 = freq, duration0 = duration)
        match waveType with
        | "sin" -> creator.sinWave
        | "square" -> creator.squareWave
        | "triangular" -> creator.triangleWave
        | "saw" -> creator.sawWave
        | _ -> creator.sinWave

    let writeToWav path data =
        writeWav().Write (File.Create(path)) (data)

    type NoteDurations =
    | double = 2.
    | full = 1.
    | half = 0.5
    | quarter = 0.25
    | eigth = 0.125


    let note duration note octav = 
        let freq = getNoteFreq octav note
        let soundData = createSound 440. 3. ""
        writeToWav "wave.wav" soundData
