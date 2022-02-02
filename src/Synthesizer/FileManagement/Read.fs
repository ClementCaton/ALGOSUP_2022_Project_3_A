namespace Synthesizer

open System
open System.IO


type ReadWav() =

    let FromBytes nbChannels bytesPerSample bytes =

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


type readMP3(stream:Stream) =

    let reader = (new BinaryReader(stream)).ReadBytes(int stream.Length)

    let rec BinaryConverter (emptArr:List<int>) byte recnum=
        if recnum > 0 then
            BinaryConverter ((byte%2)::emptArr) (byte/2) (recnum-1)
        else
            emptArr

    let FirstHeader (data:List<int>) =
        seq{0..(data.Length-2)} |> Seq.tryFindIndex(fun d -> data.[d] = 255 && data.[d+1] > 223)

    let mutable currentHeaderIndex:option<int> = FirstHeader (reader |>  Array.toList |> List.map(fun i -> int i))

    let Header (data:List<int>) =
        match currentHeaderIndex with 
        | Some int -> // AAAAAAAA AAABBCCD EEEEFFGH IIJJKLMM format
            let bit = BinaryConverter [] data.[int+1] 8 // AAABBCCD using only BBCCD
            let MPEGVer =
                            match [bit.[3]; bit.[4]] with
                            | [0;0] -> 2.5 //V2 (bis/considered as)
                            | [1;0] -> 2. // V2
                            | [1;1] -> 1. // V1
                            | [0;1] -> 0. // reserved
                            | _ -> failwith "MPEG Version not defined"
            let layerDesc = 
                            match [bit.[5]; bit.[6]] with
                            | [0;0] -> 0. // reserved
                            | [0;1] -> 3. // Layer III
                            | [1;0] -> 2. // Layer II
                            | [1;1] -> 1. // Layer I
                            | _ -> failwith "MPEG Layer not defined"
            let bitProtection = 
                                match bit.[7] with
                                | 0 -> false //  Protected by CRC (16bit crc follows header)
                                | 1 -> true 
                                | _ -> failwith "bitProtection not defined"

            let bit2 = BinaryConverter [] data.[int+2] 8 // EEEEFFGH
            let bitRateIndex =
                                match [bit2.[0];bit2.[1];bit2.[2];bit2.[3]] with
                                | [0; 0; 0; 0] -> [1; 1; 1; 1; 1] // free free free free
                                | [0; 0; 0; 1] -> [32; 32; 32; 32; 8]
                                | [0; 0; 1; 0] -> [64; 48; 40; 48; 16]
                                | [0; 0; 1; 1] -> [96; 56; 48; 56; 24]
                                | [0; 1; 0; 0] -> [128; 64; 56; 64; 32]
                                | [0; 1; 0; 1] -> [160; 80; 64; 80; 40]
                                | [0; 1; 1; 0] -> [192; 96; 80; 96; 48]
                                | [0; 1; 1; 1] -> [224; 112; 96; 112; 56]
                                | [1; 0; 0; 0] -> [256; 128; 112; 128; 64]
                                | [1; 0; 0; 1] -> [288; 160; 128; 144; 80]
                                | [1; 0; 1; 0] -> [320; 192; 160; 160; 96]
                                | [1; 0; 1; 1] -> [352; 224; 192; 176; 112]
                                | [1; 1; 0; 0] -> [384; 256; 224; 192; 128]
                                | [1; 1; 0; 1] -> [416; 320; 256; 224; 144]
                                | [1; 1; 1; 0] -> [448; 384; 320; 256; 160]
                                | [1; 1; 1; 1] -> [0; 0; 0; 0; 0] // bad bad bad bad (not permitted)
                                | _ -> failwith "bitRate not defined or non existing bitRate used"
            (*
                V1 - MPEG Version 1
                V2 - MPEG Version 2 and Version 2.5
                L1 - Layer I
                L2 - Layer II
                L3 - Layer III
            *)
            let bitRate = //cases : [ V1,L1 ;  V1,L2 ;  V1,L3 ;  V2,L1 ;  V2, L2 & L3 ]
                            match [MPEGVer; layerDesc] with
                                | [1.;1.] -> bitRateIndex.[0]
                                | [1.;2.] -> bitRateIndex.[1]
                                | [1.;3.] -> bitRateIndex.[2]
                                | [2.;1.] -> bitRateIndex.[3]
                                | [2.5;1.] -> bitRateIndex.[3]
                                | _ -> bitRateIndex.[4]
            let sampleRateFreqIndex = 
                                        match [bit2.[4]; bit2.[5]] with
                                        | [0;0] -> [44100; 22050; 11025]
                                        | [0;1] -> [48000; 24000; 12000]
                                        | [1;0] -> [44100; 22050; 11025]
                                        | [1;1] -> [0; 0; 0] // reserved
                                        | _ -> failwith "Sample rate frequency not found"
            let sampleRate =  //cases : MPEG1	MPEG2	MPEG2.5
                                match MPEGVer with
                                | 1. -> sampleRateFreqIndex.[0]
                                | 2. -> sampleRateFreqIndex.[1]
                                | 2.5 -> sampleRateFreqIndex.[2]
                                | _ -> failwith "no version found for sample rete"
            let padding = 
                            match bit2.[6] with
                            | 0 -> 0 // no padding
                            | 1 -> 1 // padded with one extra slot
                            | _ -> failwith "not a bit"

            let bit3 = BinaryConverter [] data.[int+3] 8 // IIJJKLMM
            let chanMode =
                            match [bit3.[0]; bit3.[1]] with
                            | [0;0] -> 4 // Stereo
                            | [0;1] -> 3 // Joint Stereo
                            | [1;0] -> 2 // 2 Mono channels
                            | [1;1] -> 1 // Mono
                            | _ -> failwith "non existent type of channel"
            let nextFrameHeader = ((144 * 1000 * bitRate) / sampleRate) + padding
            MPEGVer, layerDesc, bitProtection, bitRate, sampleRate, padding, chanMode, nextFrameHeader
        | None -> failwith "header not found"

    member x.mp3Decoding =
        let hData = reader |>  Array.toList |> List.map(fun i -> int i)
        let MPEGVer, layerDesc, bitProtection, bitRate, sampleRate, padding, chanMode, nextFrameHeader = Header hData
        currentHeaderIndex <- match currentHeaderIndex with
                                | Some int ->  Some (int + nextFrameHeader)
                                | _ -> failwith "next Frame Header not found"
        MPEGVer, layerDesc, bitProtection, bitRate, sampleRate, padding, chanMode, nextFrameHeader