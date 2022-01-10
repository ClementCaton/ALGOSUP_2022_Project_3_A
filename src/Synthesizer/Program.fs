namespace Synthesizer

module Program =
    let t = API.getNoteOffset 4 "C" 330
    printfn $"test= {t}" 
    /////////////////////////////////////////////////////////////////////////////////

module overD =
    let makeOverdrive multiplicator x =
        if x < (-1. * multiplicator) then (-1. * multiplicator) else
        if x > 1. * multiplicator then 1. * multiplicator else
        x



/// Write WAVE PCM soundfile




write (File.Create("toneSin.wav")) (generate fourWaves.sinWave)
write (File.Create("toneSquare.wav")) (generate fourWaves.squareWave)
write (File.Create("toneTriangle.wav")) (generate fourWaves.triangleWave)
write (File.Create("toneSaw.wav")) (generate fourWaves.sawWave)