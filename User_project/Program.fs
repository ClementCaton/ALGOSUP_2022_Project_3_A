namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let synth = Synth() // Init

    let basicSound = synth.SoundWithEnveloppe 440. (Seconds 3.) Sin 0.5 0.5 0.5 0.5 0.5     // Creating a basic sound with an envelope to make it interresting
    let repeated1 = Filter.Repeater 5 0.6 1.5 44100. basicSound
    let repeated2 = Filter.Repeater 5 0.9 4. 44100. basicSound

    synth.WriteToWav "basic.wav" [basicSound]
    synth.WriteToWav "repeated1.wav" [repeated1]
    synth.WriteToWav "repeated2.wav" [repeated2]
