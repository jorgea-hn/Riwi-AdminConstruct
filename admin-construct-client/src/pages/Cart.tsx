import { useState } from 'react';
import { Trash2 } from 'lucide-react';
import { notifications } from '../utils/notifications.ts';
import api from '../services/api';
import { useCart } from '../context/CartContext';

export default function Cart() {
  const { cartItems, removeFromCart, updateQuantity, clearCart } = useCart();
  const [loading, setLoading] = useState(false);

  // Calculate total price including rentals and products
  const calculateTotal = () => {
    return cartItems.reduce((total, item) => {
      if (item.type === 'rental' && item.startDate && item.endDate) {
        let days = Math.ceil((new Date(item.endDate).getTime() - new Date(item.startDate).getTime()) / (1000 * 60 * 60 * 24));
        if (days === 0) days = 1; // Minimum 1 day rental
        return total + (item.price * days);
      }
      return total + (item.price * item.quantity);
    }, 0);
  };

  const handleCheckout = async () => {
    if (cartItems.length === 0) {
      notifications.warning('Cart is empty');
      return;
    }

    setLoading(true);
    try {
      // 1. Get customer profile
      const customerResponse = await api.get('/customers/my-profile');
      const customerId = customerResponse.data.id;

      // 2. Separate products and rentals
      const products = cartItems.filter(item => item.type === 'product');
      const rentals = cartItems.filter(item => item.type === 'rental');

      // 3. Process product sales
      if (products.length > 0) {
        const productTotal = products.reduce((sum, item) => sum + (item.price * item.quantity), 0);
        const totalWithTax = productTotal * 1.19; // VAT 19%

        const salePayload = {
          customerId: customerId,
          totalAmount: totalWithTax,
          details: products.map(item => ({
            productId: item.id,
            quantity: item.quantity,
            unitPrice: item.price
          }))
        };

        console.log('[Cart] Creating sale:', salePayload);
        await api.post('/sales', salePayload);
      }

      // 4. Process rentals
      if (rentals.length > 0) {
        for (const rental of rentals) {
          if (!rental.startDate || !rental.endDate) continue;

          // Convert dates to ISO format with time
          const startDate = new Date(rental.startDate);
          startDate.setHours(8, 0, 0, 0); // 8:00 AM

          const endDate = new Date(rental.endDate);
          endDate.setHours(18, 0, 0, 0); // 6:00 PM

          const rentalPayload = {
            machineryId: rental.id,
            customerId: customerId,
            startDateTime: startDate.toISOString(),
            endDateTime: endDate.toISOString(),
            notes: 'Rental from web cart'
          };

          console.log('[Cart] Creating rental:', rentalPayload);
          await api.post('/machineryrental', rentalPayload);
        }
      }

      const message = products.length > 0 && rentals.length > 0
        ? 'Purchase and rental successful'
        : products.length > 0
          ? 'Purchase successful'
          : 'Rental successful';

      notifications.success(`${message}. You will receive a confirmation email.`);
      clearCart();
    } catch (error: any) {
      console.error('Error detailed:', error);
      if (error.response) {
        console.error('Response data:', error.response.data);
        console.error('Response status:', error.response.status);
      }
      notifications.error(`Error processing: ${error.response?.data?.message || error.message || 'Unknown error'}`);
    } finally {
      setLoading(false);
    }
  };

  // Determine button text based on cart content
  const getCheckoutButtonText = () => {
    const hasProducts = cartItems.some(item => item.type === 'product');
    const hasRentals = cartItems.some(item => item.type === 'rental');

    if (hasProducts && hasRentals) {
      return loading ? 'Processing...' : 'Complete Purchase and Rental';
    } else if (hasRentals) {
      return loading ? 'Processing...' : 'Complete Rental';
    } else {
      return loading ? 'Processing...' : 'Complete Purchase';
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-primary mb-8">Shopping Cart</h1>

      {cartItems.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-xl text-gray-600">Your cart is empty</p>
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
                      {item.type === 'rental' ? 'Rental' : 'Product'}
                    </p>
                    {item.type === 'rental' && item.startDate && item.endDate && (
                      <p className="text-sm text-gray-600 mt-2">
                        From {new Date(item.startDate).toLocaleDateString()} to {new Date(item.endDate).toLocaleDateString()}
                      </p>
                    )}
                    <p className="text-lg font-bold text-primary mt-2">
                      ${item.price.toFixed(2)} {item.type === 'rental' ? '/day' : ''}
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
                      onClick={() => removeFromCart(index)}
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
              <h3 className="text-xl font-bold text-gray-800 mb-4">Summary</h3>
              <div className="space-y-2 mb-4">
                <div className="flex justify-between">
                  <span className="text-gray-600">Subtotal:</span>
                  <span className="font-semibold">${calculateTotal().toFixed(2)}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">VAT (19%):</span>
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
                {getCheckoutButtonText()}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
