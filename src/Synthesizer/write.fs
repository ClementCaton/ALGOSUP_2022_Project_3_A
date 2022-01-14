namespace Synthesizer

open System.IO


type writeWav( 
        ?sampleRate0:int, 
        ?pcmFormat0:int16,
        ?nbChannels0:int,
        ?bytesPerSample0:int
        ) =

    let sampleRate = (defaultArg sampleRate0 44100) 
    let pcmFormat = (defaultArg pcmFormat0 1s)
    let nbChannels = (defaultArg nbChannels0 1)
    let bytesPerSample = (defaultArg bytesPerSample0 2)

    // let cutStart = (defaultArg cutStart 0s)
    // let cutEnd = (defaultArg cutEnd 0s)

    let bitsPerSample = bytesPerSample * 8



    member x.Write stream (data: List<float>) =
        let byteRate = sampleRate * nbChannels * bytesPerSample
        let blockAlign = uint16 (nbChannels * bytesPerSample)

        let toBytes x =
            match bytesPerSample with
            | 1 ->
                let upscaled = round ((x + 1.) * 128.)
                [ byte (upscaled - if upscaled = 255. then 1. else 0.) ]
            | 2 | 3 | 4 ->
                let maxSize = 256. ** (float bytesPerSample) / 2.
                let upscaled = round (x * maxSize)
                let corrected = upscaled - (if upscaled = maxSize then 1. else 0.)
                [ for k in 0..(bytesPerSample-1) do byte (corrected/(256.**k)) ]
            | _ -> failwithf "Invalid number of bytes per sample: %i. Valid values: 1, 2, 3, 4" bytesPerSample
        let byteData = [ for x in data do yield! toBytes x ] |> Array.ofList
        
        use writer = new BinaryWriter(stream)
        // RIFF
        writer.Write("RIFF"B)
        writer.Write(36 + byteData.Length) // File size
        writer.Write("WAVE"B)
        // fmt
        writer.Write("fmt "B)
        writer.Write(16) // Header size
        writer.Write(pcmFormat)
        writer.Write(uint16 nbChannels)
        writer.Write(sampleRate)
        writer.Write(byteRate)
        writer.Write(blockAlign)
        writer.Write(uint16 bitsPerSample)
        // data
        writer.Write("data"B)
        writer.Write(byteData.Length)
        writer.Write(byteData)
    