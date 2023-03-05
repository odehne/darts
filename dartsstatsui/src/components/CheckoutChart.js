import React, { Component } from 'react';
import { PointElement, LineElement, LinearScale, CategoryScale, Chart } from "chart.js";
import { Bar, Line, Pie } from 'react-chartjs-2';

Chart.register(CategoryScale);
Chart.register(LinearScale);
Chart.register(PointElement);
Chart.register(LineElement);

class CheckoutChart extends Component {
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
       this.populateCheckoutHistoryData();
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

    async populateCheckoutHistoryData() {
        const response = await fetch('https://localhost:7141/players/519b1df2-1b1a-4cf7-b7ce-5252bd57189d/checkouts/history');
        const data = await response.json();
        this.setState({ checkouts: data, loading: false });
    }
}

export default CheckoutChart;