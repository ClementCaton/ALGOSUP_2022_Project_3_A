open Synthesizer

module API = 
    let test = CalcNoteFreq(3, "C")
    do printfn $"test= {test.Output}" 