import React, { Component } from 'react';
import { PointElement, LineElement, LinearScale, CategoryScale, Chart } from "chart.js";
import { Line } from 'react-chartjs-2';

Chart.register(CategoryScale);
Chart.register(LinearScale);
Chart.register(PointElement);
Chart.register(LineElement);

class LegAvgChart extends Component {
    constructor(props) {
        super(props);
        this.state = {
            checkouts: {
                labels: [],
                datasets: [],
            }
        }
    }

    componentDidMount() {
        this.populateLegAvgData();
    }

    render() {
        return (
            <div className="mychart">
                <Line
                    data={this.state.checkouts}
                    width={700}
                    height={500}
                    optionas={{}}
                />
            </div>
        )
    }

    async populateLegAvgData() {
        const response = await fetch('https://localhost:7141/players/fb3fc2b2-a01b-4dd6-99e9-838262a8a614/legs/avg');
        const data = await response.json();
        this.setState({ checkouts: data, loading: false });
    }
}

export default LegAvgChart;