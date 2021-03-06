namespace Synthesizer

open System
open System.IO


type ReadWav() =



    /// <summary>
    /// Extract sound data from WAV bytes
    /// </summary>
    /// <param name="nbChannels">Number of channels in the music</param>
    /// <param name="bytesPerSample">Number of bytes for one data point</param>
    /// <param name="bytes">Data to extract from</param>
    /// <returns>Raw sound data</returns>
    
    let FromBytes (nbChannels:int) (bytesPerSample:int) (bytes:List<byte>) =

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



    /// <summary>
    /// Read data from a WAV stream
    /// </summary>
    /// <param name="stream">Stream to extract from</param>
    /// <returns>Tuple with the extracted music, duration, stream sample rate and bits per sample</returns>
    
    member x.Read stream =
        use reader = new BinaryReader(stream)

        reader.ReadBytes(20) |> ignore // ignore header ?
        let pcm = reader.ReadInt16() // ignore ?
        let nbChannels = int (reader.ReadUInt16())
        let sampleRate = reader.ReadInt32()
        let byteRate = reader.ReadInt32()
        let blockAlign = reader.ReadInt16()
        let bitsPerSample = int (reader.ReadUInt16())

        // Skip unwanted chunks
        let mutable chunkType = ""
        let mutable byteDataLength = 0
        while chunkType <> "data" do
            reader.ReadBytes(byteDataLength) |> ignore
            chunkType <- Text.Encoding.UTF8.GetString(reader.ReadBytes(4))
            byteDataLength <- reader.ReadInt32()
        
        // data
        let byteData = reader.ReadBytes(byteDataLength)
        let data = byteData |> List.ofArray |> FromBytes nbChannels (bitsPerSample/8)
        let duration = float (List.length data.[0]) / float sampleRate

        data, duration, sampleRate, bitsPerSample
