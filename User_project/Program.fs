namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let synth = Synth() // Init
    let sound1 = synth.SoundWithEnveloppe 440. (Seconds 3.) Sin 0.5 0.5 0.5 0.5 0.5  // First audio
    let sound2 = synth.SoundWithEnveloppe 440. (Seconds 3.) Sin 0.5 0.2 0.3 0.4 0.5  // Second audio

    synth.WriteToWav "stereo.wav" [sound1; sound2]  // Writing file with two channels
    