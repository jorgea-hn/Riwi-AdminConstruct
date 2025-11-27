import { Link, useLocation } from 'react-router-dom';
import { authService } from '../services/authService';
import { ShoppingCart, LogOut, User, ChevronDown, Package, Truck, UserCircle } from 'lucide-react';
import { useState, useEffect, useRef } from 'react';
import { useCart } from '../context/CartContext';

export default function Navbar() {
  const [isAuthenticated, setIsAuthenticated] = useState(authService.isAuthenticated());
  const [userEmail, setUserEmail] = useState(authService.getUserEmail());
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const location = useLocation();
  const { cartCount } = useCart();
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Actualizar estado de autenticación cuando cambia la ruta
  useEffect(() => {
    setIsAuthenticated(authService.isAuthenticated());
    setUserEmail(authService.getUserEmail());
    setIsDropdownOpen(false); // Cerrar dropdown al cambiar de ruta
  }, [location]);

  // Cerrar dropdown al hacer click fuera
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsDropdownOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [dropdownRef]);

  const handleLogout = () => {
    authService.logout();
    setIsAuthenticated(false);
    setUserEmail(null);
  };

  return (
    <nav className="bg-primary text-white shadow-lg sticky top-0 z-50">
      <div className="container mx-auto px-4">
        <div className="flex justify-between items-center h-16">
          <Link to="/" className="text-2xl font-bold hover:text-secondary transition flex items-center space-x-2">
            <div className="bg-white text-primary p-1 rounded">
              <Package size={24} />
            </div>
            <span>AdminConstruct</span>
          </Link>

          <div className="flex items-center space-x-6">
            {isAuthenticated ? (
              <>
                <Link to="/products" className="hover:text-secondary transition font-medium flex items-center space-x-1">
                  <Package size={18} />
                  <span className="hidden md:inline">Productos</span>
                </Link>
                <Link to="/machinery" className="hover:text-secondary transition font-medium flex items-center space-x-1">
                  <Truck size={18} />
                  <span className="hidden md:inline">Maquinaria</span>
                </Link>
                
                <Link to="/cart" className="relative hover:text-secondary transition group">
                  <div className="p-2 bg-primary-dark rounded-full group-hover:bg-secondary transition">
                    <ShoppingCart size={20} />
                  </div>
                  {cartCount > 0 && (
                    <span className="absolute -top-1 -right-1 bg-red-500 text-white rounded-full w-5 h-5 flex items-center justify-center text-xs font-bold border-2 border-primary">
                      {cartCount}
                    </span>
                  )}
                </Link>

                {/* User Dropdown */}
                <div className="relative" ref={dropdownRef}>
                  <button 
                    onClick={() => setIsDropdownOpen(!isDropdownOpen)}
                    className="flex items-center space-x-2 hover:bg-primary-dark px-3 py-2 rounded-lg transition focus:outline-none"
                  >
                    <div className="bg-secondary text-white p-1 rounded-full">
                      <User size={16} />
                    </div>
                    <span className="text-sm font-medium max-w-[150px] truncate hidden md:block">{userEmail}</span>
                    <ChevronDown size={16} className={`transition-transform duration-200 ${isDropdownOpen ? 'rotate-180' : ''}`} />
                  </button>

                  {isDropdownOpen && (
                    <div className="absolute right-0 mt-2 w-56 bg-white rounded-lg shadow-xl py-2 text-gray-800 border border-gray-100 animate-in fade-in slide-in-from-top-5 duration-200">
                      <div className="px-4 py-3 border-b border-gray-100 md:hidden">
                        <p className="text-sm font-medium text-gray-900 truncate">{userEmail}</p>
                      </div>
                      
                      <Link 
                        to="/profile" 
                        className="flex items-center space-x-3 px-4 py-3 hover:bg-gray-50 transition text-sm"
                        onClick={() => setIsDropdownOpen(false)}
                      >
                        <UserCircle size={18} className="text-primary" />
                        <span>Mi Perfil</span>
                      </Link>
                      
                      <div className="border-t border-gray-100 my-1"></div>
                      
                      <button
                        onClick={handleLogout}
                        className="w-full flex items-center space-x-3 px-4 py-3 hover:bg-red-50 text-red-600 transition text-sm"
                      >
                        <LogOut size={18} />
                        <span>Cerrar Sesión</span>
                      </button>
                    </div>
                  )}
                </div>
              </>
            ) : (
              <>
                <Link to="/login" className="hover:text-secondary transition font-medium">
                  Iniciar Sesión
                </Link>
                <Link
                  to="/register"
                  className="bg-secondary px-4 py-2 rounded-lg hover:bg-secondary-dark transition font-medium shadow-md hover:shadow-lg transform hover:-translate-y-0.5"
                >
                  Registrarse
                </Link>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
