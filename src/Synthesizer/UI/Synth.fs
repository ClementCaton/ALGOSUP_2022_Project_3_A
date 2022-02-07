namespace Synthesizer

open System
open System.IO

type Synth(?baseBpm:float, ?baseSampleRate:float, ?baseWaveType:BaseWaves) =

    member val bpm = defaultArg baseBpm 90.
        with get, set
    
    member val sampleRate = defaultArg baseSampleRate 44100.
        with get, set

    member val waveType = defaultArg baseWaveType Sin
        with get, set
    
    member x.GetNoteFreq octav note =
        CalcNoteFreq(octav, note).Output

    member x.GetNoteFreqOffset octav note aFourFreq =
        CalcNoteFreq(octav, note, aFourFreq).Output

    member x.Sound freq duration waveType =
        let data = SoundData(frequency0 = freq, duration0 = duration, bpm0 = x.bpm)
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.Create(waveType))

    member x.SoundWithEnveloppe freq duration waveType sustain attack hold decay release = // time, time, time, amp, time
        let data = SoundData(frequency0 = freq, duration0 = duration, bpm0 = x.bpm)
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.CreateWithEnvelope waveType sustain attack hold decay release)

    member x.SoundWithCustomEnveloppe freq duration waveType (dataPoints: List<float * float>) = // datapoints [(time, amp)]
        let data = SoundData(frequency0 = freq, duration0 = duration, bpm0 = x.bpm)
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.CreateFromDataPoints waveType dataPoints)

    member x.WriteToWav name music =
        Directory.CreateDirectory("./Output/") |> ignore
        use stream = File.Create("./Output/" + name)
        WriteWav().Write (stream) (music)

    member x.WriteToWavWithPath path fileName music =
        Directory.CreateDirectory(path) |> ignore
        use stream = File.Create(path + fileName)
        WriteWav().Write (stream) (music)

    member x.ReadFromWav name =
        ReadWav().Read (File.Open("./Output/"+name, FileMode.Open))

    member x.ReadFromWavWithPath path =
        ReadWav().Read (File.Open(path, FileMode.Open))

    member x.Note duration mNote octave =
        let freq = x.GetNoteFreq mNote octave
        x.Sound freq duration x.waveType

    member x.Silence duration =
        x.Sound 0 duration Silence
        
    member x.ComposeCutCorner (corner:int) sounds =
        sounds |> List.map(fun x -> Utility.CutCorners corner x) |> List.concat
            
    member x.Compose = x.ComposeCutCorner 100
    
    member x.ComposeNoCutCorner (sounds:list<list<float>>) = List.concat sounds
    
    member x.Add sounds = Utility.AddMean sounds

    member x.Preview title sound =
        Preview.Chart title sound
        sound

    member x.PreviewMap title map =
        map
        |> Map.toList
        |> List.unzip
        ||> Preview.ChartXY title
        map

    member x.ForAllChannels func channels =
        channels |> List.map func

    member x.Fourier sampleRate wave =
        FrequencyAnalysis.Fourier(wave)

    member x.Cutstart time (data:List<float>) =
        Utility.CutStart x.sampleRate time data

    member x.CutEnd time (data:List<float>) =
        Utility.CutEnd x.sampleRate time data

    member x.CutMiddle timeStart timeEnd (data:List<float>) =
        Utility.CutEnd x.sampleRate (float data.Length/x.sampleRate - timeStart) data @ Utility.CutStart x.sampleRate (float data.Length/x.sampleRate - timeEnd) data

    member x.CutEdge timeStart timeEnd (data:List<float>) =
        Utility.CutStart x.sampleRate timeStart (Utility.CutEnd x.sampleRate timeEnd data)
        
    member x.CutCorners limit (data:List<float>) =
        Utility.CutCorners limit data

    member x.ApplyFilters filters data =
        Filter.ApplyFilters filters data

    member x.PlayWav (offset:float32) data =
        match int Environment.OSVersion.Platform with
        | 4| 6 -> 
            x.WriteToWavWithPath "./Output/temp_file_storage/" ".tempFile.wav" data
            PlayMusic.PlayMac "./Output/temp_file_storage/.tempFile.wav" offset |> ignore
            File.Delete "./Output/temp_file_storage/.tempFile.wav"
            Directory.Delete "./Output/temp_file_storage/" |> ignore
        | _       ->  
            let stream = new MemoryStream()
            WriteWav().Write stream data 
            PlayMusic.PlayWithOffset offset stream |> ignore

    member x.PlayWavFromPath offset (filePath:string) =
        match int Environment.OSVersion.Platform with
        | 4| 6 -> 
            PlayMusic.PlayMac filePath offset |> ignore
        | _ ->  
            PlayMusic.PlayWithOffsetFromPath offset filePath
