namespace Synthesizer

module Program =
    let t = API.getNoteOffset 4 "C" 330
    printfn $"test= {t}" 