namespace Synthesizer

open System.IO


type WriteWav( 
        ?SampleRate0:int, 
        ?PcmFormat0:int,
        ?BytesPerSample0:int
        ) =

    let SampleRate = (defaultArg SampleRate0 44100)
    let PcmFormat = (defaultArg PcmFormat0 1)
    let BytesPerSample = (defaultArg BytesPerSample0 2)

    // let CutStart = (defaultArg cutStart 0s)
    // let CutEnd = (defaultArg cutEnd 0s)

    let BitsPerSample = BytesPerSample * 8



    member x.Write Stream (data: List<List<float>>) =
        if PcmFormat <> 1 then failwithf "Invalid pcm format %i" PcmFormat

        let NbChannels = List.length Data
        let ByteRate = SampleRate * NbChannels * BytesPerSample
        let BlockAlign = uint16 (NbChannels * BytesPerSample)

        let ToBytes x =
            match BytesPerSample with
            | 1 ->
                let Upscaled = Round ((x + 1.) * 128.)
                [| Byte (Upscaled - if Upscaled = 255. then 1. else 0.) |]
            | 2 | 3 | 4 ->
                let MaxSize = 256. ** (float BytesPerSample) / 2.
                let Upscaled = Round (x * MaxSize)
                let Corrected = Upscaled - (if Upscaled = MaxSize then 1. else 0.)
                [| for k in 0..(BytesPerSample-1) do Byte (Corrected/(256.**k)) |]
            | _ -> failwithf "Invalid number of bytes per sample: %i. Valid values: 1, 2, 3, 4" BytesPerSample
        
        let Transposed = Data |> List.transpose
        let ByteData = [| for Sample in Transposed do yield! [| for Channel in Sample do yield! ToBytes Channel |] |]

        
        let Transposed = Data |> List.transpose
        let ByteData = [| for Sample in Transposed do yield! [| for Channel in Sample do yield! ToBytes Channel |] |]

        let Encode = new System.Text.UTF32Encoding() 
        use Writer = new BinaryWriter(Stream, Encode, true)
        // RIFF
        writer.Write("RIFF"B)
        writer.Write(36 + ByteData.Length) // File size
        writer.Write("WAVE"B)
        // fmt
        writer.Write("fmt "B)
        writer.Write(16) // Header size
        writer.Write(uint16 PcmFormat)
        writer.Write(uint16 NbChannels)
        writer.Write(SampleRate)
        writer.Write(ByteRate)
        writer.Write(BlockAlign)
        writer.Write(uint16 BitsPerSample)
        // data
        writer.Write("data"B)
        writer.Write(ByteData.Length)
        writer.Write(ByteData)
    