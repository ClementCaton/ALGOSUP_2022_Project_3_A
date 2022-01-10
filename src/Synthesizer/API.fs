namespace Synthesizer

open System
open System.IO

module API = 
    
    let getNote octav note =
        let output = CalcNoteFreq(octav, note)
        output.Output

    let getNoteOffset octav note aFourFreq =
        let output = CalcNoteFreq(octav, note, aFourFreq)
        output.Output

    let createSound =
        let creator = createSoundData()
        creator.sin

    let writeToWav path data =
        let writer = writeWav()
        writer.Write (File.Create(path)) (data)
