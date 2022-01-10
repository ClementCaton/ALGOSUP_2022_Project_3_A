namespace Synthesizer

module API = 
    
    let getNote octav note =
        let output = CalcNoteFreq(octav, note)
        output.Output

    let getNoteOffset octav note aFourFreq =
        let output = CalcNoteFreq(octav, note, aFourFreq)
        output.Output

    //let getSinData