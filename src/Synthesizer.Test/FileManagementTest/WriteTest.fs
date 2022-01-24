module Synthesizer.writeTest

open NUnit.Framework
open System
open System.IO

[<SetUp>]
let Setup () =
    ()

[<Test>]
let Write () =
    let stream = new MemoryStream()
    let data = new List<float>()
    for i in 0..100 do
        data.[i] <- 0.

    let val1 = writeWav().Write stream data

    use reader = new BinaryReader(stream)

    reader.ReadBytes(20) |> ignore // ignore header ?
    let pcm = reader.ReadInt16() // ignore ?
    let nbChannels = int (reader.ReadUInt16())
    let sampleRate = reader.ReadInt32()
    let byteRate = reader.ReadInt32()
    let blockAlign = reader.ReadInt16()
    let bitsPerSample = int (reader.ReadUInt16())
    // data
    reader.ReadBytes(4) |> ignore
    let byteDataLength = reader.ReadInt32()
    let byteData = reader.ReadBytes(byteDataLength)

    let data = byteData |> fromBytes (bitsPerSample/8)
    let duration = float (Array.length data) / float sampleRate

    Asssert.That((data |> Array.toList).GetType,Is.TypeOf<List<float>>)
