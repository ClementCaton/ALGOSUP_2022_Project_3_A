namespace Synthesizer

open System
open System.IO


type ReadWav() =

    let FromBytes NbChannels BytesPerSample Bytes =

        match BytesPerSample with
        | 1 -> [List.map (fun b -> (float b) / 255. * 2. - 1.) bytes]
        | _ ->
            bytes
            |> List.chunkBySize (BytesPerSample * NbChannels) // Split in samples
            |> List.map (List.ChunkBySize BytesPerSample) // Split each samples in channels
            |> List.transpose // Now channels of samples
            |> List.map (
                List.map List.indexed
                >> List.map (List.fold (fun v (k, b) -> v + (float b) * (256. ** float k)) 0.)
                >> List.map (fun x -> x / 256. ** (float BytesPerSample))
                >> List.map (fun x -> (x * 2. + 1.) % 2. - 1.)
            )

    member x.Read Stream =
        use Reader = new BinaryReader(Stream)

        Reader.ReadBytes(20) |> ignore // ignore header ?
        let Pcm = Reader.ReadInt16() // ignore ?
        let NbChannels = int (Reader.ReadUInt16())
        let SampleRate = Reader.ReadInt32()
        let ByteRate = Reader.ReadInt32()
        let BlockAlign = Reader.ReadInt16()
        let BitsPerSample = int (Reader.ReadUInt16())

        // Skip unwanted chunks
        let mutable ChunkType = ""
        let mutable ByteDataLength = 0
        while ChunkType <> "data" do
            Reader.ReadBytes(ByteDataLength) |> ignore
            ChunkType <- Text.Encoding.UTF8.GetString(Reader.ReadBytes(4))
            ByteDataLength <- Reader.ReadInt32()
        
        // data
        let ByteData = Reader.ReadBytes(ByteDataLength)
        let Data = ByteData |> List.ofArray |> FromBytes NbChannels (BitsPerSample/8)
        let Duration = float (List.length data.[0]) / float SampleRate

        Data, Duration, SampleRate, BitsPerSample