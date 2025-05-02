import React from "react";
import { MdStore } from "react-icons/md";
import logoFuria from "./Images/logo-furia.svg"
import { Link } from "react-router-dom";

function NavBar() {
    return (
        <nav className="navBar">
            <div>
                <p>FuriaFan Insights</p>
            </div>

            <div>
                <Link to="./">
                    <img src={logoFuria} alt="Logo Furia" />
                </Link>
            </div>

            <div>
                <a href="https://www.furia.gg/" target="_blank">
                    <p className="iconStore"><MdStore /></p>
                </a>
            </div>
        </nav>



    )
}

export default NavBar;