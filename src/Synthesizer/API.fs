namespace Synthesizer

open System
open System.IO

module API = 

    //let note =

    ////////////////////////////////////////////////////////////////////////
    
    let getNoteFreq octav note =
        CalcNoteFreq(octav, note).Output

    let getNoteFreqOffset octav note aFourFreq =
        CalcNoteFreq(octav, note, aFourFreq).Output

    let createSound =
        let creator = createSoundData(overdrive0 = 0.8, duration0 = 3.)
        creator.sawWave

    let writeToWav path data =
        writeWav().Write (File.Create(path)) (data)
