namespace Synthesizer

open System.IO

module Program =

    let basicSound = API.note (Seconds 0.5) Note.A 4
    let reverb = Filter.reverb 50 0.8 0.2 44100. [] basicSound
    let echo = Filter.reverb 5 0.7 1. 44100. [] basicSound
    API.writeToWav "basic.wav" basicSound
    API.writeToWav "reverb.wav" reverb
    API.writeToWav "echo.wav" echo
