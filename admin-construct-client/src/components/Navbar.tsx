import { Link, useLocation } from 'react-router-dom';
import { authService } from '../services/authService';
import { ShoppingCart, LogOut, User } from 'lucide-react';
import { useState, useEffect } from 'react';

interface NavbarProps {
  cartItemCount?: number;
}

export default function Navbar({ cartItemCount = 0 }: NavbarProps) {
  const [isAuthenticated, setIsAuthenticated] = useState(authService.isAuthenticated());
  const [userEmail, setUserEmail] = useState(authService.getUserEmail());
  const location = useLocation();

  // Actualizar estado de autenticación cuando cambia la ruta
  useEffect(() => {
    setIsAuthenticated(authService.isAuthenticated());
    setUserEmail(authService.getUserEmail());
  }, [location]);

  const handleLogout = () => {
    authService.logout();
  };

  return (
    <nav className="bg-primary text-white shadow-lg">
      <div className="container mx-auto px-4">
        <div className="flex justify-between items-center h-16">
          <Link to="/" className="text-2xl font-bold hover:text-secondary transition">
            AdminConstruct
          </Link>

          <div className="flex items-center space-x-6">
            {isAuthenticated ? (
              <>
                <Link to="/products" className="hover:text-secondary transition font-medium">
                  Productos
                </Link>
                <Link to="/machinery" className="hover:text-secondary transition font-medium">
                  Maquinaria
                </Link>
                <Link to="/profile" className="hover:text-secondary transition font-medium">
                  Mi Perfil
                </Link>
                <Link to="/cart" className="relative hover:text-secondary transition">
                  <ShoppingCart size={24} />
                  {cartItemCount > 0 && (
                    <span className="absolute -top-2 -right-2 bg-secondary text-white rounded-full w-5 h-5 flex items-center justify-center text-xs font-bold">
                      {cartItemCount}
                    </span>
                  )}
                </Link>
                <div className="flex items-center space-x-2 text-sm">
                  <User size={20} />
                  <span>{userEmail}</span>
                </div>
                <button
                  onClick={handleLogout}
                  className="flex items-center space-x-1 hover:text-secondary transition font-medium"
                >
                  <LogOut size={20} />
                  <span>Salir</span>
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="hover:text-secondary transition font-medium">
                  Iniciar Sesión
                </Link>
                <Link
                  to="/register"
                  className="bg-secondary px-4 py-2 rounded-lg hover:bg-secondary-dark transition font-medium"
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
