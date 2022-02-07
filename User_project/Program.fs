namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let synth = new Synth()
    
    let sound = synth.SoundWithEnveloppe 440. (Seconds 3.) Sin 0.5 0.5 0.5 0.5 0.5
    synth.WriteToWav "env.wav" [sound]

