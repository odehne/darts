import React, { Component } from 'react';
import { BarElement, Title, LinearScale, CategoryScale, Tooltip, Legend, Chart } from "chart.js";
import { Bar } from 'react-chartjs-2';

Chart.register(
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
);

class WonVsLostChart extends Component {
    constructor(props) {
        super(props);
        this.state = {
            wonsVsLosses: {
                labels: [],
                datasets: [],
            }
        }
    }

    componentDidMount() {
        this.populateWinsAndLossesData();
    }

    render() {
        return (
            <div className="wonvslostchart">
                <Bar
                    data={this.state.wonsVsLosses}
                    width={700}
                    height={500}
                    options={{}}
                />
            </div>
        )
    }

    async populateWinsAndLossesData() {
        const response = await fetch('https://localhost:7141/players/fb3fc2b2-a01b-4dd6-99e9-838262a8a614/legs/winsandlosses');
        const data = await response.json();
        this.setState({ wonsVsLosses: data, loading: false });
    }
}

export default WonVsLostChart;