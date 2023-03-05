import React, { Component } from 'react';
import { PointElement, LineElement, LinearScale, CategoryScale, Chart } from "chart.js";
import { Line } from 'react-chartjs-2';

Chart.register(CategoryScale);
Chart.register(LinearScale);
Chart.register(PointElement);
Chart.register(LineElement);


class TotalDartsChart extends Component {
    constructor(props) {
        super(props);
        this.state = {
            totalDartsData: {
                labels: [],
                datasets: [],
            }
        }
    }

    componentDidMount() {
        this.populateTotalDartsData();
    }

    render() {
        return (
            <div className="mychart">
                <Line
                    data={this.state.totalDartsData}
                    width={700}
                    height={500}
                    optionas={{}}
                />
            </div>
        )
    }

    async populateTotalDartsData() {
        const response = await fetch('https://localhost:7141/players/519b1df2-1b1a-4cf7-b7ce-5252bd57189d/legs/darts');
        const data = await response.json();
        this.setState({ totalDartsData: data, loading: false });
    }
}

export default TotalDartsChart;