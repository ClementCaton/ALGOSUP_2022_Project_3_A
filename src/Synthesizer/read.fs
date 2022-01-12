module read 
    open System
    open System.IO  
    let read stream =
        use reader = new BinaryReader(stream)

        reader.ReadBytes(20) |> ignore // Header
        let pcm = reader.ReadInt16()
        let nbChannels = reader.ReadUInt16()
        let sampleRate = reader.ReadInt32()
        let byteRate = reader.ReadInt32()
        let blockAlign = reader.ReadInt16()
        let bitsPerSample = reader.ReadUInt16()
        // data
        reader.ReadBytes(4) |> ignore
        let dataLength = reader.ReadInt32()
        let data = reader.ReadBytes(dataLength)

        let duration = float dataLength / float sampleRate
        sampleRate, duration, nbChannels,bitsPerSample, data

    let readAndReturnStream stream =
        use reader = new BinaryReader(stream)

        reader.ReadBytes(20) |> ignore // Header
        let pcm = reader.ReadInt16()
        let nbChannels = reader.ReadUInt16()
        let sampleRate = reader.ReadInt32()
        let bitsPerSample = reader.ReadUInt16()
        // data
        reader.ReadBytes(4) |> ignore
        let dataLength = reader.ReadInt32()
        let data = reader.ReadBytes(dataLength)

        let bytePerSample = bitsPerSample/ uint16 8
        let writer = new Synthesizer.writeWav(sampleRate, pcm, int nbChannels, int bytePerSample)
        let memStream = new MemoryStream()
        writer.Write memStream data
        
        memStream