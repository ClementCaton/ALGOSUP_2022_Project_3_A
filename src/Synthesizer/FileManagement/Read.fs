namespace Synthesizer

open System.IO


type readWav() =

    let fromBytes nbChannels bytesPerSample bytes =

        match bytesPerSample with
        | 1 -> [List.map (fun b -> (float b) / 255. * 2. - 1.) bytes]
        | _ ->
            bytes
            |> List.chunkBySize (bytesPerSample * nbChannels) // Split in samples
            |> List.map (List.chunkBySize bytesPerSample) // Split each samples in channels
            |> List.transpose // Now channels of samples
            |> List.map (
                List.map List.indexed
                >> List.map (List.fold (fun v (k, b) -> v + (float b) * (256. ** float k)) 0.)
                >> List.map (fun x -> x / 256. ** (float bytesPerSample))
                >> List.map (fun x -> (x * 2. + 1.) % 2. - 1.)
            )

    member x.Read stream =
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
    
        let data = byteData |> List.ofArray |> fromBytes nbChannels (bitsPerSample/8)
        let duration = float (List.length data.[0]) / float sampleRate

        data, duration, sampleRate, bitsPerSample