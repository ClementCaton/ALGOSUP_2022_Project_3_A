namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let synth = Synth() // Init

    let basic = synth.Note (Seconds 2.) Note.A 4
    let modWave = synth.Sound 100. (Seconds 2.) Sin
    let fm = Filter.LFO_FM modWave 2. basic
    synth.WriteToWav "basic.wav" [basic]
    synth.WriteToWav "modWave.wav" [modWave]
    synth.WriteToWav "fm.wav" [fm]

