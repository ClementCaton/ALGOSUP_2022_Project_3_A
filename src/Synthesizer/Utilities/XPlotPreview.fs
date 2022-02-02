namespace Synthesizer
open XPlot
open XPlot.Plotly.Layout

module Preview =

    let Chart title (y:List<float>) =
        y |> Plotly.Chart.Line |> Plotly.Chart.WithOptions(Plotly.Options(title = title)) |> Plotly.Chart.Show
    
    let ChartXY title (x:List<float>) (y:List<float>) =
        Plotly.Scatter(x=x, y=y) |> Plotly.Chart.Plot |> Plotly.Chart.WithLayout(Layout(title=title)) |> Plotly.Chart.Show
