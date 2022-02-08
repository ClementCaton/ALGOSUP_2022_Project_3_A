namespace Synthesizer
open XPlot
open XPlot.Plotly.Layout

module Preview =

    /// <summary>
    /// Creates a chart plotting the given data list
    /// </summary>
    /// <param name="title">Title of the plot</param>
    /// <param name="y">List of float</param>
    
    let Chart (title:string) (y:List<float>) =
        y |> Plotly.Chart.Line |> Plotly.Chart.WithOptions(Plotly.Options(title = title)) |> Plotly.Chart.Show
    


    /// <summary>
    /// Creates a chart plotting the given x and y values
    /// </summary>
    /// <param name="title">Title of the plot</param>
    /// <param name="x">List of the X coordinates</param>
    /// <param name="y">List of the y coordinates</param>
    
    let ChartXY title (x:List<float>) (y:List<float>) =
        Plotly.Scatter(x=x, y=y) |> Plotly.Chart.Plot |> Plotly.Chart.WithLayout(Layout(title=title)) |> Plotly.Chart.Show
