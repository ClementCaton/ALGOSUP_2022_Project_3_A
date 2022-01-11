namespace Synthesizer

module Program =
    let t = API.getNote 4 "a"
    printfn $"test= {t}" 
    let t2 = "b".ToUpper()
    printfn $"{t2}"