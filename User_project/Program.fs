﻿namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let d = (new Synth()).ReadFromMP3 "440.mp3"
    printfn "%A" (d) 