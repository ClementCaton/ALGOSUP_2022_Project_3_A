namespace Synthesizer

open System
open System.IO

module API =
    
    let getNoteFreq octav note =
        CalcNoteFreq(octav, note).Output

    let getNoteFreqOffset octav note aFourFreq =
        CalcNoteFreq(octav, note, aFourFreq).Output

    let createSound freq duration overdrive waveType =
        let data = createSoundData(overDrive0 = overdrive ,frequency0 = freq, duration0 = duration, bpm0 = 114) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.makeOverdrive overdrive (data.create(waveType))

    let createSoundWithEnveloppe freq duration overdrive waveType sustain attack hold decay release = // time, time, time, amp, time
        let data = createSoundData(overDrive0 = overdrive ,frequency0 = freq, duration0 = duration, bpm0 = 114) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.makeOverdrive overdrive (data.creteWithEnvelope waveType sustain attack hold decay release)

    let writeToWav name music =
        Directory.CreateDirectory("./Output/") |> ignore
        use stream = File.Create("./Output/" + name)
        writeWav().Write (stream) (music)

    let writeToWavWithPath path fileName music =
        Directory.CreateDirectory(path) |> ignore
        use stream = File.Create(path + fileName)
        writeWav().Write (stream) (music)

    let readFromWav name =
        readWav().Read (File.Open("./Output/"+name, FileMode.Open))

    let readFromWavWithPath path =
        readWav().Read (File.Open(path, FileMode.Open))

    let note duration mNote octave =
        let freq = getNoteFreq mNote octave
        createSound freq duration 1. Sin
    
    let silence duration =
        createSound 0 duration 1. Silence
    
    let compose sounds =
        //this is to be revisited
        sounds |> List.map(fun x -> Utility.cutCorners 3500 x) |> List.concat
            
    let add sounds = Utility.add sounds

    let preview title sound =
        previewarr.chart title sound
        sound

    let forAllChannels func channels =
        channels |> List.map func

    let fourier wave =
        frequencyAnalysis.fourier(wave)
