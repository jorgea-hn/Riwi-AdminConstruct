import { useState, useEffect } from 'react';
import { Trash2 } from 'lucide-react';
import { notifications } from '../utils/notifications.ts';

interface CartItem {
  id: string | number;
  name: string;
  price: number;
  quantity: number;
  type: 'product' | 'rental';
  startDate?: string;
  endDate?: string;
}

export default function Cart() {
  const [cartItems, setCartItems] = useState<CartItem[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadCart();
  }, []);

  const loadCart = () => {
    const savedCart = JSON.parse(localStorage.getItem('cart') || '[]');
    setCartItems(savedCart);
  };

  const removeItem = (index: number) => {
    const newCart = cartItems.filter((_, i) => i !== index);
    setCartItems(newCart);
    localStorage.setItem('cart', JSON.stringify(newCart));
  };

  const updateQuantity = (index: number, newQuantity: number) => {
    if (newQuantity < 1) return;
    const newCart = [...cartItems];
    newCart[index].quantity = newQuantity;
    setCartItems(newCart);
    localStorage.setItem('cart', JSON.stringify(newCart));
  };

  const calculateTotal = () => {
    return cartItems.reduce((total, item) => {
      if (item.type === 'rental' && item.startDate && item.endDate) {
        const days = Math.ceil((new Date(item.endDate).getTime() - new Date(item.startDate).getTime()) / (1000 * 60 * 60 * 24));
        return total + (item.price * days);
      }
      return total + (item.price * item.quantity);
    }, 0);
  };

  const handleCheckout = async () => {
    if (cartItems.length === 0) {
      notifications.warning('El carrito está vacío');
      return;
    }

    setLoading(true);
    try {
      // Aquí puedes implementar la lógica de checkout
      // Por ahora solo limpiamos el carrito
      notifications.success('Compra realizada con éxito. Recibirás un correo de confirmación.');
      localStorage.removeItem('cart');
      setCartItems([]);
    } catch (error) {
      console.error('Error:', error);
      notifications.error('Error al procesar la compra');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-primary mb-8">Carrito de Compras</h1>

      {cartItems.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-xl text-gray-600">Tu carrito está vacío</p>
        </div>
      ) : (
        <div className="grid lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2 space-y-4">
            {cartItems.map((item, index) => (
              <div key={index} className="bg-white rounded-lg shadow-md p-6">
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <h3 className="text-lg font-semibold text-gray-800">{item.name}</h3>
                    <p className="text-sm text-gray-500 mt-1">
                      {item.type === 'rental' ? 'Alquiler' : 'Producto'}
                    </p>
                    {item.type === 'rental' && item.startDate && item.endDate && (
                      <p className="text-sm text-gray-600 mt-2">
                        Del {new Date(item.startDate).toLocaleDateString()} al {new Date(item.endDate).toLocaleDateString()}
                      </p>
                    )}
                    <p className="text-lg font-bold text-primary mt-2">
                      ${item.price.toFixed(2)} {item.type === 'rental' ? '/día' : ''}
                    </p>
                  </div>
                  
                  <div className="flex items-center space-x-4">
                    {item.type === 'product' && (
                      <div className="flex items-center space-x-2">
                        <button
                          onClick={() => updateQuantity(index, item.quantity - 1)}
                          className="px-2 py-1 bg-gray-200 rounded hover:bg-gray-300"
                        >
                          -
                        </button>
                        <span className="px-4">{item.quantity}</span>
                        <button
                          onClick={() => updateQuantity(index, item.quantity + 1)}
                          className="px-2 py-1 bg-gray-200 rounded hover:bg-gray-300"
                        >
                          +
                        </button>
                      </div>
                    )}
                    <button
                      onClick={() => removeItem(index)}
                      className="text-red-500 hover:text-red-700"
                    >
                      <Trash2 size={20} />
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>

          <div className="lg:col-span-1">
            <div className="bg-white rounded-lg shadow-md p-6 sticky top-4">
              <h3 className="text-xl font-bold text-gray-800 mb-4">Resumen</h3>
              <div className="space-y-2 mb-4">
                <div className="flex justify-between">
                  <span className="text-gray-600">Subtotal:</span>
                  <span className="font-semibold">${calculateTotal().toFixed(2)}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">IVA (19%):</span>
                  <span className="font-semibold">${(calculateTotal() * 0.19).toFixed(2)}</span>
                </div>
                <div className="border-t pt-2 flex justify-between text-lg font-bold">
                  <span>Total:</span>
                  <span className="text-primary">${(calculateTotal() * 1.19).toFixed(2)}</span>
                </div>
              </div>
              <button
                onClick={handleCheckout}
                disabled={loading}
                className="w-full bg-secondary text-white py-3 rounded-lg hover:bg-orange-600 transition disabled:opacity-50"
              >
                {loading ? 'Procesando...' : 'Finalizar Compra'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
