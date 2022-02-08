namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let synth = Synth()
    let basicSound = synth.SoundWithEnveloppe 440. (Seconds 1.) Sin 0.5 0.2 0.2 0.2 0.2 // Creating a basic sound with an envelope to make it interesting
    let flanger = Filter.Flanger 20. 0.4 44100. 114. basicSound
    
    synth.WriteToWav "basic.wav" [basicSound]
    synth.WriteToWav "flanger.wav" [flanger]
