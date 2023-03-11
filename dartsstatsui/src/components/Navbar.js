import React, {Component} from 'react';
import * as FaIcons from 'react-icons/fa';
import * as AiIcons from 'react-icons/ai';
import * as IoIcons from 'react-icons/io';
import { Link } from 'react-router-dom';
import './Navbar.css';
import { IconContext } from 'react-icons';


class Navbar extends Component {

    constructor(props) {
        super(props);
        this.state = {
            sidebar: false,
            sidebarData: {
                players: [
                    {
                        name: 'olli',
                        id: 'myid',
                        path: '/player'
                    }
                ],
            }
        }
    }

    componentDidMount() {
        this.populatePlayers();
    }

    render() {

        return (
            <>
                <IconContext.Provider value={{ color: '#fff' }}>
                    <div className="navbar">
                        <link to="#" className="menu-bars"></link>
                        <FaIcons.FaBars onClick={() => this.setState({ sidebar: !this.state.sidebar })} />
                    </div>
                    <nav className={this.state.sidebar ? 'nav-menu active' : 'nav-menu'} >
                        <ul className='nav-menu-items' onClick={() => this.setState({ sidebar: !this.state.sidebar })}>
                            <li className='navbar-toggle'>
                                <Link to='#' className='menu-bars'>
                                    <AiIcons.AiOutlineClose />
                                </Link>
                            </li>
                            {this.state.sidebarData.players.map((player) => {
                                return (
                                    <li key={player.id} className='nav-text'>
                                        <Link to={player.path}>
                                            <IoIcons.IoMdPeople />
                                            <span>{player.name}</span>
                                        </Link>
                                    </li>
                                )
                            })}
                        </ul>
                    </nav>
                </IconContext.Provider>
            </>
        )
    }

    async populatePlayers() {
        const response = await fetch('https://localhost:7141/players');
        const data = await response.json();

        this.setState({ sidebarData: data });
        this.setState({ loading: false })
        this.setState({ sidebar: false });
    }
  
}

export default Navbar;