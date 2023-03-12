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
                <Bar
                    data={this.state.checkouts}
                    width={500}
                    height={300}
                    optionas={{}}
                />
            </div>
        )
    }

    async populateCheckoutHistoryData() {
        const response = await fetch('https://localhost:7141/players/fb3fc2b2-a01b-4dd6-99e9-838262a8a614/checkouts/history');
        const data = await response.json();
        this.setState({ checkouts: data, loading: false });
    }
}

export default CheckoutChart;