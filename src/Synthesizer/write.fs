namespace Synthesizer

open System.IO


type writeWav(                      //not tested this one yet
        //?freq0:int, // In Hertz
        //:?amplitude0:float,
        //?overdrive0:float,
        //?duration0:float, // In seconds
        ?sampleRate0:int, // In Hertz
        ?pcmFormat0:int16,
        ?nbChannels0:int,
        ?bytesPerSample0:int
        ) =

    //let freq = (defaultArg freq0 440) // In Hertz
    //let amplitude = (defaultArg amplitude0 0.8)
    //let overdrive = (defaultArg overdrive0 0.9)
    //let duration = (defaultArg duration0 1.) // In secondss
    let sampleRate = (defaultArg sampleRate0 44100) // In Hertz
    let pcmFormat = (defaultArg pcmFormat0 1s)
    let nbChannels = (defaultArg nbChannels0 1)
    let bytesPerSample = (defaultArg bytesPerSample0 2)

    let bitsPerSample = bytesPerSample * 8



    member x.Write stream (data: byte []) =
        let byteRate = sampleRate * nbChannels * bytesPerSample
        let blockAlign = uint16 (nbChannels * bytesPerSample)
        
        use writer = new BinaryWriter(stream)
        // RIFF
        writer.Write("RIFF"B)
        writer.Write(36 + data.Length) // File size
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
        writer.Write(data.Length)
        writer.Write(data)


    (* let generate func =
        let size = int (duration * float sampleRate)
        let toBytes x =
            let unitary = (x + 2. ** (float bytesPerSample - 1.)) / 2.
            let upscaled = round (unitary * (256. ** (float bytesPerSample))) - (if unitary = 1. then 1. else 0.)
            [ for k in 0..(bytesPerSample-1) do byte (upscaled/(256.**k)) ]
        
        let getData = float >> (fun x -> (x / float sampleRate)) >> func freq amplitude >> overD.makeOverdrive overdrive >> toBytes
        [ for i in 0 .. (size - 1) do yield! getData i ] |> Array.ofList
    *)
    