namespace Synthesizer

open System.IO

module Program =

    let basicSound = API.note (Seconds 3) Note.A 4
    let flanger = Filter.primitiveFlanger 10 44100. basicSound
    // let reverb = Filter.reverb basicSound 50 0.8 0.2 44100.
    // let echo = Filter.reverb basicSound 5 0.7 1. 44100.
    API.writeToWav "basic.wav" basicSound
    API.writeToWav "flanger.wav" flanger
    // API.writeToWav "reverb.wav" reverb
    // API.writeToWav "echo.wav" echo
