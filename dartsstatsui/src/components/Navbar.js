import React from 'react';
import { useEffect, useState } from 'react';
import * as FaIcons from 'react-icons/fa';
import * as AiIcons from 'react-icons/ai';
import * as IoIcons from 'react-icons/io';
import { Link } from 'react-router-dom';
import './Navbar.css';
import { IconContext } from 'react-icons';


function Navbar(props) {

    const [sidebar, setSidebar] = useState(false); 
    const [navbarState, setSidebarData] = useState({
        players: [
            {
                name: 'olli',
                id: 'myid',
                path: '/player'
            }
        ],
    });

    useEffect(() => {
        async function fetchData() {
            let mounted = true;
            let url = 'https://localhost:7141/players/';
            await fetch(url)
                .then(response => response.json())
                .then(response => {
                    if (mounted) {
                        setSidebarData(response)
                    }
                })
                .catch(error => console.log('error: ' + error))
            return () => mounted = false;
        }
        fetchData();
    }, [])

    return (
        <IconContext.Provider value={{ color: '#fff' }}>
            <div className="navbar">
                <link to="#" className="menu-bars"></link>
                <FaIcons.FaBars onClick={() => { setSidebar(!sidebar) }} />
            </div>
            <nav className={sidebar ? 'nav-menu' : 'nav-menu active'} >
                <ul className='nav-menu-items' onClick={() => { setSidebar(!sidebar) }}>
                    <li className='navbar-toggle'>
                        <Link to='#' className='menu-bars'>
                            <AiIcons.AiOutlineClose />
                        </Link>
                    </li>
                    {navbarState.players.map((player) => {
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
    )
}

export default Navbar;